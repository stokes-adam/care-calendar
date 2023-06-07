import axios, { AxiosError, AxiosRequestConfig } from 'axios';
import { tap, catchError, from, map, Observable, of } from 'rxjs';
import { Result, ok, err } from './result';

export function get<T>(url: string, config?: AxiosRequestConfig): Observable<Result<T, any>> {
  return from(axios.get<T>(url, config)).pipe(
    map((response) => ok(response.data)),
    catchError((error: AxiosError) => of(err(error))),
  );
}

export function post<T>(url: string, data?: any, config?: AxiosRequestConfig): Observable<Result<T, any>> {
  return from(axios.post<T>(url, data, config)).pipe(
    map((response) => ok(response.data)),
    catchError((error: AxiosError) => of(err(error))),
  );
}
