import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router : Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        if(error){
          switch(error.status){
            case 400:
              console.log(error);
              if(error.error.errors){
                const modalStateErrors = [];
                for(const key in error.error.errors){
                  if(error.error.errors[key]){
                    modalStateErrors.push(error.error.errors[key]);
                  }
                }
                throw modalStateErrors.flat();
              }
              else {
                this.toastr.error("Bad Request", error.status);
              }
              break;
            case 401:
              this.toastr.error("Unauthorized", error.status)
              break;
            case 404:
              this.router.navigateByUrl('/not-found');
              break;
            case 500:
              const navigateExtras : NavigationExtras = {state:{error: error.error}};
              this.router.navigateByUrl('/server-error', navigateExtras);
              break;
            default:
              this.toastr.error('something went to wrong!');
            break;
          }
        }
        return throwError(() =>error);
      })
    )
  }
}
