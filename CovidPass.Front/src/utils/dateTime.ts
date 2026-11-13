// © 2020 Partners HealthCare Technology & Innovation Department
import parseISO from 'date-fns/parseISO';
import formatFn from 'date-fns/format';

export class DateTime {
  public static epicInit(dateTime: Date): Date {
    return parseISO(dateTime as any);
  }

  public static parse(dateTime: string) {
    const value = parseISO(dateTime);
    return value;
  }

  public static format(
    date: Date,
    format?: 'day' | 'date' | 'datetime' | 'day and datetime' | 'iso' | 'isodate' | 'time' | 'shortMonth' | 'shortDay'
  ) {
    let useFormat: string;
    switch (format) {
      case 'shortMonth':
        useFormat = 'MMM';
        break;
      case 'day':
        useFormat = 'EEEE';
        break;
      case 'shortDay':
        useFormat = 'EEE';
        break;
      case 'date':
        useFormat = 'MM/dd/yyyy';
        break;
      case 'datetime':
        useFormat = 'MM/dd/yyyy HH:mm:ss';
        break;
      case 'day and datetime':
        useFormat = 'EEEE MM/dd/yyyy HH:mm:ss';
        break;
      case 'time':
        useFormat = 'pp';
        break;
      case 'iso':
        useFormat = "yyyy-MM-dd'T'HH:mm:ss.SSSxxx";
        break;
      case 'isodate':
        useFormat = 'yyyy-MM-dd';
        break;
      default:
        useFormat = 'MM/dd/yyyy HH:mm';
        break;
    }
    const value = formatFn(date, useFormat);
    return value;
  }

  public static getStartOfWeek(date: Date) {
    const startOfWeek = new Date(date.getFullYear(), date.getMonth(), date.getDate());

    while (startOfWeek.getDay() !== 0) {
      startOfWeek.setDate(startOfWeek.getDate() - 1);
    }
    return startOfWeek;
  }
}
