import { Routes } from '@angular/router';

export const CUSTOMER_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/customer-page/customer-page.component').then(
        (module) => module.CustomerPageComponent
      )
  }
];
