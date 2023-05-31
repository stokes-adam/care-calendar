import { Map } from './map';

export type DynamicTableData<T extends Map> = {
  columns: (keyof T)[];
  rows: T[]; 
}

export function toDynamicTableData<T extends Map>(data: T[]): DynamicTableData<T> {
  if (data.length === 0) {
    return { columns: [], rows: [] };
  }

  const keys = Object.keys(data[0]) as (keyof T)[];

  const rows = data;

  return { columns: keys, rows };
}