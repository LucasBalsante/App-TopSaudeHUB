import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';

import { API_BASE_URL } from '@core/tokens/api-base-url.token';

const ABSOLUTE_URL_PATTERN = /^https?:\/\//i;

export const apiBaseUrlInterceptor: HttpInterceptorFn = (request, next) => {
  if (ABSOLUTE_URL_PATTERN.test(request.url)) {
    return next(request);
  }

  const apiBaseUrl = inject(API_BASE_URL);
  const normalizedBaseUrl = apiBaseUrl.endsWith('/') ? apiBaseUrl : `${apiBaseUrl}/`;
  const normalizedPath = request.url.startsWith('/') ? request.url.slice(1) : request.url;

  return next(
    request.clone({
      url: `${normalizedBaseUrl}${normalizedPath}`
    })
  );
};