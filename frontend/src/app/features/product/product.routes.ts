import { Routes } from '@angular/router';

export const PRODUCT_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/product-page/product-page.component').then(
        (module) => module.ProductPageComponent
      )
  }
];
