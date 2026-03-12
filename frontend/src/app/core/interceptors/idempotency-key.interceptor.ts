import { HttpInterceptorFn } from '@angular/common/http';

import { IDEMPOTENCY_ENABLED, IDEMPOTENCY_KEY } from '@core/tokens/idempotency-key.token';

const IDEMPOTENCY_REQUIRED_METHOD = 'POST';

export const idempotencyKeyInterceptor: HttpInterceptorFn = (request, next) => {
  const isEnabled = request.context.get(IDEMPOTENCY_ENABLED);
  const key = request.context.get(IDEMPOTENCY_KEY);

  if (
    !isEnabled ||
    request.method !== IDEMPOTENCY_REQUIRED_METHOD ||
    !key ||
    request.headers.has('Idempotency-Key')
  ) {
    return next(request);
  }

  return next(
    request.clone({
      headers: request.headers.set('Idempotency-Key', key)
    })
  );
};