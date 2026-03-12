import { Routes } from '@angular/router';
import { APP_ROUTES } from '@core/constants/app-routes';

export const routes: Routes = [
  {
    path: APP_ROUTES.home,
    loadChildren: () => import('@features/home/home.routes').then((module) => module.HOME_ROUTES)
  },
  {
    path: APP_ROUTES.customers,
    loadChildren: () =>
      import('@features/customer/customer.routes').then((module) => module.CUSTOMER_ROUTES)
  },
  {
    path: APP_ROUTES.products,
    loadChildren: () =>
      import('@features/product/product.routes').then((module) => module.PRODUCT_ROUTES)
  },
  {
    path: '**',
    redirectTo: APP_ROUTES.home,
    pathMatch: 'full'
  }
];
