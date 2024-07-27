import { queryOptions } from "@tanstack/react-query";
import { notFound } from "@tanstack/react-router";

export default function getOptions<T>(uri: string, queryKey: string[]) {
  return queryOptions({
    // eslint-disable-next-line @tanstack/query/exhaustive-deps
    queryKey,
    queryFn: async () => {
      const response = await fetch(uri, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
      });

      if (!response.ok) {
        if (response.status === 404) {
          // eslint-disable-next-line @typescript-eslint/no-throw-literal
          throw notFound();
        }
        throw new Error("Network error");
      }

      return response.json() as Promise<T>;
    },
    staleTime: 10 * 60 * 1000,
    throwOnError: true,
  });
}
