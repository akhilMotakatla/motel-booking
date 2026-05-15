import {
  Component, OnInit, OnDestroy, ElementRef, ViewChild,
  AfterViewInit, signal, inject, HostListener
} from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import * as THREE from 'three';
import { RoomService } from '../../core/services/room.service';
import { Room } from '../../core/models/room.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule, DecimalPipe],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('heroCanvas') heroCanvas!: ElementRef<HTMLCanvasElement>;
  @ViewChild('heroSection') heroSection!: ElementRef<HTMLElement>;

  private roomService = inject(RoomService);
  private fb = inject(FormBuilder);
  private router = inject(Router);

  /* ── State ── */
  featuredRooms = signal<Room[]>([]);
  isLoading = signal(true);
  activeGuideStep = signal(0);
  showGuide = signal(false);
  guideVisible = signal(false);
  mouseX = signal(0);
  mouseY = signal(0);
  currentImageIndex: Record<number, number> = {};

  /* ── Search Form ── */
  readonly today = new Date().toISOString().split('T')[0];

  searchForm = this.fb.group({
    checkIn: ['', Validators.required],
    checkOut: ['', Validators.required],
    guests: [1, [Validators.min(1), Validators.max(10)]]
  });

  /* ── Three.js ── */
  private renderer!: THREE.WebGLRenderer;
  private scene!: THREE.Scene;
  private camera!: THREE.PerspectiveCamera;
  private particles!: THREE.Points;
  private stars!: THREE.Points;
  private animFrameId = 0;
  private clock = new THREE.Clock();

  /* ── Virtual Guide Steps ── */
  readonly guideSteps = [
    { icon: '👋', title: 'Welcome!', message: 'I\'m your personal guide. Let me show you around our luxury motel!', targetId: '' },
    { icon: '🛏️', title: 'Stunning Rooms', message: 'Browse our collection of premium rooms — from cozy standards to lavish penthouse suites.', targetId: 'rooms-section' },
    { icon: '🔑', title: 'Easy Booking', message: 'Sign in or create an account to book your perfect room in just minutes.', targetId: 'booking-section' },
    { icon: '⭐', title: 'World-Class Amenities', message: 'Enjoy WiFi, spa, room service, and so much more during your stay.', targetId: 'amenities-section' },
    { icon: '📞', title: 'Always Here', message: 'Our front desk is available 24/7. Contact us anytime!', targetId: 'contact' }
  ];

  /* ── Stats ── */
  readonly stats = [
    { value: '500+', label: 'Happy Guests Monthly', icon: 'group' },
    { value: '4.9★', label: 'Average Rating', icon: 'star' },
    { value: '50+', label: 'Premium Rooms', icon: 'king_bed' },
    { value: '24/7', label: 'Guest Support', icon: 'support_agent' }
  ];

  /* ── Amenities ── */
  readonly amenities = [
    { icon: 'wifi', name: 'High-Speed WiFi', desc: 'Blazing fast gigabit internet throughout' },
    { icon: 'local_bar', name: 'Premium Mini Bar', desc: 'Curated selection of fine beverages' },
    { icon: 'spa', name: 'Spa & Wellness', desc: 'Full-service spa and fitness center' },
    { icon: 'room_service', name: '24/7 Room Service', desc: 'Gourmet dining delivered to your door' },
    { icon: 'local_parking', name: 'Valet Parking', desc: 'Complimentary secure valet service' },
    { icon: 'pool', name: 'Rooftop Pool', desc: 'Infinity pool with panoramic city views' }
  ];

  ngOnInit(): void {
    this.loadFeaturedRooms();
    const hasVisited = sessionStorage.getItem('mb_guide_shown');
    if (!hasVisited) {
      setTimeout(() => {
        this.showGuide.set(true);
        setTimeout(() => this.guideVisible.set(true), 100);
        sessionStorage.setItem('mb_guide_shown', '1');
      }, 2000);
    }
  }

  ngAfterViewInit(): void {
    this.initThreeJS();
    this.initScrollObserver();
  }

  ngOnDestroy(): void {
    cancelAnimationFrame(this.animFrameId);
    this.renderer?.dispose();
  }

  @HostListener('mousemove', ['$event'])
  onMouseMove(e: MouseEvent): void {
    this.mouseX.set((e.clientX / window.innerWidth - 0.5) * 2);
    this.mouseY.set((e.clientY / window.innerHeight - 0.5) * 2);
  }

  /* ── Three.js Hero ── */
  private initThreeJS(): void {
    const canvas = this.heroCanvas.nativeElement;
    const w = canvas.offsetWidth || window.innerWidth;
    const h = canvas.offsetHeight || window.innerHeight;

    this.scene = new THREE.Scene();
    this.camera = new THREE.PerspectiveCamera(75, w / h, 0.1, 1000);
    this.camera.position.z = 5;

    this.renderer = new THREE.WebGLRenderer({ canvas, alpha: true, antialias: true });
    this.renderer.setSize(w, h);
    this.renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
    this.renderer.setClearColor(0x000000, 0);

    this.createStarField();
    this.createFloatingParticles();
    this.createNebulaEffect();
    this.animate();

    window.addEventListener('resize', () => this.onCanvasResize());
  }

  private createStarField(): void {
    const geometry = new THREE.BufferGeometry();
    const count = 3000;
    const positions = new Float32Array(count * 3);
    const sizes = new Float32Array(count);

    for (let i = 0; i < count; i++) {
      positions[i * 3] = (Math.random() - 0.5) * 80;
      positions[i * 3 + 1] = (Math.random() - 0.5) * 80;
      positions[i * 3 + 2] = (Math.random() - 0.5) * 80;
      sizes[i] = Math.random() * 1.5;
    }

    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    geometry.setAttribute('size', new THREE.BufferAttribute(sizes, 1));

    const material = new THREE.PointsMaterial({
      color: 0xffffff,
      size: 0.08,
      transparent: true,
      opacity: 0.7,
      sizeAttenuation: true
    });

    this.stars = new THREE.Points(geometry, material);
    this.scene.add(this.stars);
  }

  private createFloatingParticles(): void {
    const geometry = new THREE.BufferGeometry();
    const count = 200;
    const positions = new Float32Array(count * 3);
    const colors = new Float32Array(count * 3);

    const goldColors = [
      [0.83, 0.69, 0.22],
      [0.96, 0.78, 0.26],
      [0.72, 0.53, 0.04],
      [1, 0.84, 0.2]
    ];

    for (let i = 0; i < count; i++) {
      positions[i * 3] = (Math.random() - 0.5) * 20;
      positions[i * 3 + 1] = (Math.random() - 0.5) * 20;
      positions[i * 3 + 2] = (Math.random() - 0.5) * 20;
      const c = goldColors[Math.floor(Math.random() * goldColors.length)];
      colors[i * 3] = c[0];
      colors[i * 3 + 1] = c[1];
      colors[i * 3 + 2] = c[2];
    }

    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    geometry.setAttribute('color', new THREE.BufferAttribute(colors, 3));

    const material = new THREE.PointsMaterial({
      size: 0.15,
      vertexColors: true,
      transparent: true,
      opacity: 0.9,
      sizeAttenuation: true,
      blending: THREE.AdditiveBlending,
      depthWrite: false
    });

    this.particles = new THREE.Points(geometry, material);
    this.scene.add(this.particles);
  }

  private createNebulaEffect(): void {
    const geometry = new THREE.SphereGeometry(12, 32, 32);
    const material = new THREE.MeshBasicMaterial({
      color: 0x1a0a00,
      transparent: true,
      opacity: 0.03,
      side: THREE.BackSide
    });
    const nebula = new THREE.Mesh(geometry, material);
    this.scene.add(nebula);
  }

  private animate(): void {
    this.animFrameId = requestAnimationFrame(() => this.animate());
    const t = this.clock.getElapsedTime();

    if (this.stars) {
      this.stars.rotation.y = t * 0.02;
      this.stars.rotation.x = t * 0.005;
    }

    if (this.particles) {
      this.particles.rotation.y = t * 0.05;
      this.particles.rotation.z = t * 0.02;

      const positions = this.particles.geometry.attributes['position'].array as Float32Array;
      for (let i = 0; i < positions.length; i += 3) {
        positions[i + 1] += Math.sin(t + i) * 0.001;
      }
      this.particles.geometry.attributes['position'].needsUpdate = true;
    }

    this.camera.position.x += (this.mouseX() * 0.5 - this.camera.position.x) * 0.05;
    this.camera.position.y += (-this.mouseY() * 0.3 - this.camera.position.y) * 0.05;
    this.camera.lookAt(this.scene.position);

    this.renderer.render(this.scene, this.camera);
  }

  private onCanvasResize(): void {
    const canvas = this.heroCanvas.nativeElement;
    const w = canvas.offsetWidth;
    const h = canvas.offsetHeight;
    this.camera.aspect = w / h;
    this.camera.updateProjectionMatrix();
    this.renderer.setSize(w, h);
  }

  /* ── Scroll Observer ── */
  private initScrollObserver(): void {
    const observer = new IntersectionObserver(
      entries => entries.forEach(e => {
        if (e.isIntersecting) e.target.classList.add('revealed');
      }),
      { threshold: 0.15, rootMargin: '0px 0px -80px 0px' }
    );
    document.querySelectorAll('.reveal').forEach(el => observer.observe(el));
  }

  /* ── Search ── */
  onSearch(): void {
    if (this.searchForm.invalid) {
      this.searchForm.markAllAsTouched();
      return;
    }
    const { checkIn, checkOut, guests } = this.searchForm.value;
    this.router.navigate(['/rooms'], {
      queryParams: { checkIn, checkOut, guests }
    });
  }

  /* ── Guide ── */
  nextGuideStep(): void {
    const next = this.activeGuideStep() + 1;
    if (next >= this.guideSteps.length) {
      this.dismissGuide();
    } else {
      this.activeGuideStep.set(next);
      this.scrollToGuideTarget();
    }
  }

  dismissGuide(): void {
    this.guideVisible.set(false);
    setTimeout(() => this.showGuide.set(false), 400);
  }

  private scrollToGuideTarget(): void {
    const step = this.guideSteps[this.activeGuideStep()];
    if (step.targetId) {
      document.getElementById(step.targetId)?.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
  }

  /* ── Rooms ── */
  private loadFeaturedRooms(): void {
    this.roomService.getFeaturedRooms().subscribe({
      next: rooms => {
        this.featuredRooms.set(rooms);
        rooms.forEach(r => this.currentImageIndex[r.id] = 0);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  getRoomImage(room: Room): string {
    const idx = this.currentImageIndex[room.id] ?? 0;
    return room.imageUrls[idx] || room.thumbnailUrl || 'assets/images/room-placeholder.jpg';
  }

  nextRoomImage(room: Room, e: Event): void {
    e.preventDefault();
    e.stopPropagation();
    const max = room.imageUrls.length - 1;
    const cur = this.currentImageIndex[room.id] ?? 0;
    this.currentImageIndex[room.id] = cur < max ? cur + 1 : 0;
  }

  prevRoomImage(room: Room, e: Event): void {
    e.preventDefault();
    e.stopPropagation();
    const max = room.imageUrls.length - 1;
    const cur = this.currentImageIndex[room.id] ?? 0;
    this.currentImageIndex[room.id] = cur > 0 ? cur - 1 : max;
  }

  getStars(rating: number): number[] {
    return Array(Math.round(rating)).fill(0);
  }

  getStatusBadge(status: string): string {
    const map: Record<string, string> = {
      'Available': 'badge-success',
      'Occupied': 'badge-error',
      'Maintenance': 'badge-warning',
      'Reserved': 'badge-info'
    };
    return map[status] ?? 'badge-info';
  }
}
