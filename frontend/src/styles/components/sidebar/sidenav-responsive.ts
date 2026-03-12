import { MediaMatcher } from '@angular/cdk/layout';
import { NgFor } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnDestroy, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';

interface NavigationItem {
  title: string;
  link: string;
}

/** @title Responsive sidenav */
@Component({
  selector: 'sidenav-responsive',
  standalone: true,
  templateUrl: 'sidenav-responsive.html',
  styleUrl: 'sidenav-responsive.css',
  imports: [NgFor, MatToolbarModule, MatButtonModule, MatIconModule, MatSidenavModule, MatListModule],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidenavResponsive implements OnDestroy {
  protected readonly navigationItems: NavigationItem[] = [
    { title: 'Home', link: '/' },
    { title: 'Clientes', link: '/clientes' },
    { title: 'Produtos', link: '/produtos' }
  ];

  protected readonly isMobile = signal(true);

  private readonly _mobileQuery: MediaQueryList;
  private readonly _mobileQueryListener: () => void;

  constructor() {
    const media = inject(MediaMatcher);

    this._mobileQuery = media.matchMedia('(max-width: 600px)');
    this.isMobile.set(this._mobileQuery.matches);
    this._mobileQueryListener = () => this.isMobile.set(this._mobileQuery.matches);
    this._mobileQuery.addEventListener('change', this._mobileQueryListener);
  }

  ngOnDestroy(): void {
    this._mobileQuery.removeEventListener('change', this._mobileQueryListener);
  }

  protected trackByLink(_index: number, item: NavigationItem): string {
    return item.link;
  }
}