import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { UserService } from '../../../core/services/user.service';
import { NotificationService } from '../../../core/services/notification.service';
import { User } from '../../../core/models/user.model';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './user-management.component.html',
  styleUrls: ['../room-management/room-management.component.scss']
})
export class UserManagementComponent implements OnInit {
  private userService = inject(UserService);
  private notify = inject(NotificationService);

  users = signal<User[]>([]);
  isLoading = signal(true);
  deleteConfirmId = signal<number | null>(null);

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading.set(true);
    this.userService.getAllUsers().subscribe({
      next: u => { this.users.set(u); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  confirmDelete(id: number): void { this.deleteConfirmId.set(id); }
  cancelDelete(): void { this.deleteConfirmId.set(null); }

  deleteUser(id: number): void {
    this.userService.deleteUser(id).subscribe({
      next: () => { this.notify.success('User removed.'); this.deleteConfirmId.set(null); this.load(); },
      error: () => this.notify.error('Delete failed.')
    });
  }

  getRoleBadge(role: string): string {
    return role === 'Admin' ? 'badge-gold' : 'badge-info';
  }
}
