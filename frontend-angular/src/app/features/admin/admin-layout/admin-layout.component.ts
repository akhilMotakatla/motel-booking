import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.scss']
})
export class AdminLayoutComponent {
  sidebarOpen = signal(true);

  readonly navItems = [
    { icon: 'dashboard', label: 'Dashboard', route: '/admin/dashboard' },
    { icon: 'king_bed', label: 'Rooms', route: '/admin/rooms' },
    { icon: 'book_online', label: 'Bookings', route: '/admin/bookings' },
    { icon: 'people', label: 'Users', route: '/admin/users' },
  ];

  constructor(public authService: AuthService) {}

  toggle(): void { this.sidebarOpen.update(v => !v); }
}
