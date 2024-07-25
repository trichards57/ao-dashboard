import { useMutation, useQueryClient } from "@tanstack/react-query";
import { notFound } from "@tanstack/react-router";

export function useUpdate(uri: string, queryKey: string[]) {
  const queryClient = useQueryClient();

  return function useUpdate<TBody>(id: string) {
    return useMutation({
      mutationFn: async (body: TBody) => {
        const response = await fetch(`${uri}/${id}`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(body),
        });

        if (!response.ok) {
          if (response.status === 404) {
            throw notFound();
          }
          throw new Error("Failed to update.");
        }
      },
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey });
      },
    });
  };
}
