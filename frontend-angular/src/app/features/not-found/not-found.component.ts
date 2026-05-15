import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="nf-page">
      <div class="nf-content">
        <div class="nf-code text-gradient">404</div>
        <h1 class="nf-title">Page Not Found</h1>
        <p class="nf-sub">The room you're looking for seems to have checked out. Let us guide you back.</p>
        <div class="nf-actions">
          <a routerLink="/" class="btn-primary">
            <span class="material-icons">home</span> Back to Home
          </a>
          <a routerLink="/rooms" class="btn-outline">
            <span class="material-icons">hotel</span> Browse Rooms
          </a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .nf-page {
      min-height: 100vh; display: flex; align-items: center;
      justify-content: center; text-align: center; padding: 24px;
    }
    .nf-code {
      font-family: 'Playfair Display', serif;
      font-size: clamp(6rem, 20vw, 12rem); font-weight: 900;
      line-height: 1; margin-bottom: 16px;
    }
    .nf-title {
      font-family: 'Playfair Display', serif;
      font-size: clamp(1.5rem, 4vw, 2.5rem); font-weight: 900;
      color: #f8f8f8; margin-bottom: 16px;
    }
    .nf-sub {
      color: rgba(248,248,248,0.5); font-size: 1rem;
      max-width: 400px; margin: 0 auto 36px; line-height: 1.7;
    }
    .nf-actions { display: flex; gap: 14px; justify-content: center; flex-wrap: wrap; }
  `]
})
export class NotFoundComponent {}
