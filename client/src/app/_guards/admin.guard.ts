import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { Observable, map } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

export const adminGuard: CanActivateFn = () : Observable<boolean> => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService); 
  return accountService.currentUser$.pipe(
    map(user => {
      if(user.roles.includes('Admin') || user.roles.includes('Superadmin')) 
      {return true;}
      else{
        toastr.error("You can not access this area!")
        return false
      }
    })
  )
};
