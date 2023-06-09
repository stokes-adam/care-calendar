export type Result<T, E> = Ok<T> | Err<E>;

export type Ok<T> = { type: 'ok'; value: T };
export type Err<E> = { type: 'err'; error: E };

export const ok = <T>(value: T): Ok<T> => ({ type: 'ok', value });
export const err = <E>(error: E): Err<E> => ({ type: 'err', error });

export const isOk = <T, E>(result: Result<T, E>): result is Ok<T> => result.type === 'ok';
export const isErr = <T, E>(result: Result<T, E>): result is Err<E> => result.type === 'err';

export const map = <T, E, U>(result: Result<T, E>, fn: (value: T) => U): Result<U, E> =>
  isOk(result) ? ok(fn(result.value)) : result;

export const mapErr = <T, E, F>(result: Result<T, E>, fn: (error: E) => F): Result<T, F> =>
  isErr(result) ? err(fn(result.error)) : result;

export const andThen = <T, E, U>(result: Result<T, E>, fn: (value: T) => Result<U, E>): Result<U, E> =>
  isOk(result) ? fn(result.value) : result;

export const orElse = <T, E, F>(result: Result<T, E>, fn: (error: E) => Result<T, F>): Result<T, F> =>
  isErr(result) ? fn(result.error) : result;
