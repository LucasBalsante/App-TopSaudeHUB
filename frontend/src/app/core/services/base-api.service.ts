import { HttpClient, HttpContext } from '@angular/common/http';
import { inject } from '@angular/core';

import { buildIdempotencyContext } from '@core/tokens/idempotency-key.token';

export abstract class BaseApiService {
  protected readonly http = inject(HttpClient);

  protected buildPath(path: string): string {
    return path.startsWith('/') ? path.slice(1) : path;
  }

  protected buildMutationContext(idempotencyKey?: string): HttpContext {
    return buildIdempotencyContext(idempotencyKey);
  }
}