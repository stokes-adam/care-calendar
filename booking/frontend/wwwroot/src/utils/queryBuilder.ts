export const queryBuilder = (
  url: string,
  params: Record<string, string> = {}
): string => {
  let urlParams = Object.entries(params)
    .map(([key, value]) => `${key}=${value}`)
    .join("&");

  if (urlParams.length > 0) urlParams = `?${urlParams}`;

  return `${import.meta.env.VITE_API_URL}/${url}${urlParams}`;
};
