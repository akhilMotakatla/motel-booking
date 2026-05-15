import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { RoomService } from '../../../core/services/room.service';
import { LocationService } from '../../../core/services/location.service';
import { Room, PagedResult, RoomFilter } from '../../../core/models/room.model';
import { StateInfo, CityInfo, Branch } from '../../../core/models/location.model';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-room-list',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './room-list.component.html',
  styleUrls: ['./room-list.component.scss']
})
export class RoomListComponent implements OnInit {
  private roomService = inject(RoomService);
  private locationService = inject(LocationService);
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);
  public authService = inject(AuthService);

  result = signal<PagedResult<Room> | null>(null);
  isLoading = signal(true);
  showFilters = signal(false);
  viewMode = signal<'grid' | 'list'>('grid');
  tiltStates: Record<number, { x: number; y: number }> = {};

  /* ── Location Cascade ── */
  states = signal<StateInfo[]>([]);
  cities = signal<CityInfo[]>([]);
  branches = signal<Branch[]>([]);
  selectedState = signal<string>('');
  selectedCity = signal<string>('');
  selectedBranch = signal<Branch | null>(null);
  locationLoading = signal(false);

  filter: RoomFilter = {
    pageNumber: 1, pageSize: 12, sortBy: 'Price', sortDescending: false
  };

  filterForm = this.fb.group({
    search: [''],
    minPrice: [null],
    maxPrice: [null],
    maxOccupancy: [null],
    sortBy: ['Price'],
    sortDescending: [false]
  });

  readonly roomTypes = [
    { id: null, name: 'All Types' },
    { id: 1, name: 'Standard' },
    { id: 2, name: 'Deluxe' },
    { id: 3, name: 'Suite' },
    { id: 4, name: 'Executive' },
    { id: 5, name: 'Family' }
  ];
  selectedTypeId: number | null = null;

  ngOnInit(): void {
    this.locationService.getStates().subscribe(s => this.states.set(s));

    this.route.queryParams.subscribe(params => {
      if (params['checkIn']) this.filter.checkInDate = params['checkIn'];
      if (params['checkOut']) this.filter.checkOutDate = params['checkOut'];
      if (params['guests']) this.filter.maxOccupancy = +params['guests'];
      this.loadRooms();
    });

    this.filterForm.get('search')!.valueChanges.pipe(
      debounceTime(400), distinctUntilChanged()
    ).subscribe(val => {
      this.filter.search = val ?? undefined;
      this.filter.pageNumber = 1;
      this.loadRooms();
    });
  }

  /* ── Location Cascade Handlers ── */
  onStateChange(state: string): void {
    this.selectedState.set(state);
    this.selectedCity.set('');
    this.selectedBranch.set(null);
    this.cities.set([]);
    this.branches.set([]);
    this.filter.locationId = undefined;

    if (!state) { this.loadRooms(); return; }

    this.locationLoading.set(true);
    this.locationService.getCities(state).subscribe({
      next: c => { this.cities.set(c); this.locationLoading.set(false); }
    });
    this.loadRooms();
  }

  onCityChange(city: string): void {
    this.selectedCity.set(city);
    this.selectedBranch.set(null);
    this.branches.set([]);
    this.filter.locationId = undefined;

    if (!city) { this.loadRooms(); return; }

    this.locationLoading.set(true);
    this.locationService.getBranches(this.selectedState(), city).subscribe({
      next: b => { this.branches.set(b); this.locationLoading.set(false); }
    });
    this.loadRooms();
  }

  onBranchChange(branch: Branch | null): void {
    this.selectedBranch.set(branch);
    this.filter.locationId = branch?.id ?? undefined;
    this.filter.pageNumber = 1;
    this.loadRooms();
  }

  clearLocation(): void {
    this.selectedState.set('');
    this.selectedCity.set('');
    this.selectedBranch.set(null);
    this.cities.set([]);
    this.branches.set([]);
    this.filter.locationId = undefined;
    this.loadRooms();
  }

  /* ── Room Loading ── */
  loadRooms(): void {
    this.isLoading.set(true);
    const f = this.filterForm.value;
    this.roomService.getRooms({
      ...this.filter,
      search: f.search || undefined,
      minPrice: f.minPrice || undefined,
      maxPrice: f.maxPrice || undefined,
      maxOccupancy: f.maxOccupancy || undefined,
      roomTypeId: this.selectedTypeId || undefined,
      sortBy: f.sortBy || 'Price',
      sortDescending: f.sortDescending || false
    }).subscribe({
      next: r => { this.result.set(r); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  applyFilters(): void {
    this.filter.pageNumber = 1;
    this.loadRooms();
    this.showFilters.set(false);
  }

  clearFilters(): void {
    this.filterForm.reset({ search: '', sortBy: 'Price', sortDescending: false });
    this.selectedTypeId = null;
    this.filter = { pageNumber: 1, pageSize: 12, sortBy: 'Price', sortDescending: false };
    this.clearLocation();
  }

  selectType(id: number | null): void {
    this.selectedTypeId = id;
    this.filter.pageNumber = 1;
    this.loadRooms();
  }

  changePage(p: number): void {
    this.filter.pageNumber = p;
    this.loadRooms();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  get pages(): number[] {
    return Array.from({ length: this.result()?.totalPages ?? 1 }, (_, i) => i + 1);
  }

  onCardMouseMove(e: MouseEvent, roomId: number): void {
    const card = e.currentTarget as HTMLElement;
    const rect = card.getBoundingClientRect();
    this.tiltStates[roomId] = {
      x: ((e.clientY - rect.top) / rect.height - 0.5) * 14,
      y: -((e.clientX - rect.left) / rect.width - 0.5) * 14
    };
  }

  onCardMouseLeave(roomId: number): void {
    this.tiltStates[roomId] = { x: 0, y: 0 };
  }

  getTiltStyle(id: number): string {
    const t = this.tiltStates[id] ?? { x: 0, y: 0 };
    return `perspective(1000px) rotateX(${t.x}deg) rotateY(${t.y}deg)`;
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      Available: 'badge-success', Occupied: 'badge-error',
      Maintenance: 'badge-warning', Reserved: 'badge-info'
    };
    return map[status] ?? 'badge-info';
  }

  getStars(r: number): number[] { return Array(Math.round(r)).fill(0); }
  get rooms(): Room[] { return this.result()?.items ?? []; }
  get totalCount(): number { return this.result()?.totalCount ?? 0; }

  get locationSummary(): string {
    if (this.selectedBranch()) return this.selectedBranch()!.branchName;
    if (this.selectedCity()) return `${this.selectedCity()}, ${this.selectedState()}`;
    if (this.selectedState()) return this.selectedState();
    return '';
  }
}
