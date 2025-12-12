import { ApplicationConfig, inject } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { AuthService } from './services/auth.service';
import { tokenInterceptor } from './services/token.interceptor';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';

export const appConfig: ApplicationConfig = {
  providers: [
   provideHttpClient(
      withInterceptors([tokenInterceptor])
    ),
    provideRouter(routes),
    provideClientHydration(withEventReplay())
  ]
};
