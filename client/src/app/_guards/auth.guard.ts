import { CanActivateFn } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

export const authGuard: CanActivateFn = () : Observable<boolean> =>  {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService); 
  return accountService.currentUser$.pipe(
    map(user => {
      if(user) 
      {return true;}
      else{
        toastr.error("You shall not access!")
        return false
      }
    })
  )
};


