import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmDialogComponent } from '../modal/confirm-dialog/confirm-dialog.component';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  bsMdodalRef: BsModalRef;

  constructor(private modalService : BsModalService) { }

  confirm(title = 'Confirmation',
  message = 'Are you sure you want to do this?',
  btnOkText = 'Ok',
  btnCancelText = 'Cancel'): Observable<boolean> {
    const config ={
      initialState : {
        title,
        message,
        btnOkText,
        btnCancelText
      }
    }
    this.bsMdodalRef = this.modalService.show(ConfirmDialogComponent, config);

    return new Observable<boolean>(this.getResult());
  }

  private getResult(){
    return (observer) =>{
      const subscription = this.bsMdodalRef.onHidden?.subscribe(() => {
        observer.next(this.bsMdodalRef.content.result);
        observer.complete();
      });

      return {
        unsubscribe() {
          subscription.unsubscribe();
        }
      }
    }
  }
}
