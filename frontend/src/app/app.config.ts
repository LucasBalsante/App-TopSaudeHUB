import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

import { apiBaseUrlInterceptor } from '@core/interceptors/api-base-url.interceptor';
import { idempotencyKeyInterceptor } from '@core/interceptors/idempotency-key.interceptor';
import { API_BASE_URL } from '@core/tokens/api-base-url.token';
import { routes } from './app.routes';
import { environment } from 'environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideAnimationsAsync(),
    provideHttpClient(withInterceptors([apiBaseUrlInterceptor, idempotencyKeyInterceptor])),
    {
      provide: API_BASE_URL,
      useValue: environment.apiBaseUrl
    }
  ]
};
