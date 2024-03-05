import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'datePipe'
})
export class DatePipe implements PipeTransform {

  transform(value: number, ...args: unknown[]): string {
      let dtt = new Date(value * 1000).toDateString();
      return dtt;
  }

}
