//TODO: Make locale dynamic
export class DateHelper {
  static ToDateTimeString(date: Date | null): string {
    if (date === null) return 'Algum dia'
    return new Date(date).toLocaleString('pt-br', {
      day: '2-digit',
      year: '2-digit',
      month: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      hourCycle: 'h23',
      hour12: false
    })
  }
  static ToDateString(date: Date | null): string {
    if (date === null) return 'Algum dia'
    return new Date(date).toLocaleString('pt-br', {
      day: '2-digit',
      year: '2-digit',
      month: '2-digit'
    })
  }
  static ToTimeString(date: Date | null): string {
    if (date === null) return 'Alguma hora'
    return new Date(date).toLocaleString('pt-br', {
      hour: '2-digit',
      minute: '2-digit',
      hourCycle: 'h23',
      hour12: false
    })
  }
}
