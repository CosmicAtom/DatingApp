import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { Observable } from 'rxjs';
import { inject } from '@angular/core';
import { ConfirmService } from '../_services/confirm.service';

export const preventUnsavedChangesGuard: CanDeactivateFn<unknown> = (component:MemberEditComponent): boolean | Observable<boolean> => {
  const confirmService = inject(ConfirmService);

  if(component.editForm.dirty){
    return confirmService.confirm();
  }
  return true;
};