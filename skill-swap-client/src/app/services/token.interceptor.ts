import { HttpInterceptorFn } from '@angular/common/http';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('jwt');  // simple + no injection issues
  console.log("TOKEN:", token);

  const cloned = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  console.log("REQUEST AFTER INTERCEPTOR:", cloned);

  return next(cloned);
};
