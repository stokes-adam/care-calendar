import axios from "axios";
import { Observable, from, map } from "rxjs";
import { Either, tryCatch } from "fp-ts/Either";

export const get = <T>(url: string): Observable<Either<Error, T>> =>
  from(axios.get<T>(url)).pipe(
    map((response) =>
      tryCatch(
        () => response.data,
        () => new Error(response.statusText)
      )
    )
  );

export const post = <T, Payload = any>(
  url: string,
  data: Payload
): Observable<Either<Error, T>> =>
  from(axios.post<T>(url, data)).pipe(
    map((response) =>
      tryCatch(
        () => response.data,
        () => new Error(response.statusText)
      )
    )
  );

export const put = <T, Payload = any>(
  url: string,
  data: Payload
): Observable<Either<Error, T>> =>
  from(axios.put<T>(url, data)).pipe(
    map((response) =>
      tryCatch(
        () => response.data,
        () => new Error(response.statusText)
      )
    )
  );

export const del = <T>(url: string): Observable<Either<Error, T>> =>
  from(axios.delete<T>(url)).pipe(
    map((response) =>
      tryCatch(
        () => response.data,
        () => new Error(response.statusText)
      )
    )
  );
