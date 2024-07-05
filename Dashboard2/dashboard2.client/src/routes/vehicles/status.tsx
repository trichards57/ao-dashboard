import { createFileRoute, redirect, useSearch } from "@tanstack/react-router";
import PlacePicker from "../../components/place-picker";
import { statusOptions } from "../../queries/vor-queries";
import { useSuspenseQuery } from "@tanstack/react-query";
import { useState } from "react";
import PagePicker from "../../components/page-picker";

const PageSize = 10;

const VehicleStatus = () => {
  const { region, district, hub } = useSearch({ from: "/vehicles/status" }) as {
    region: string;
    district: string;
    hub: string;
  };
  const { data } = useSuspenseQuery(statusOptions(region, district, hub));
  const [page, setPage] = useState(0);

  const itemsToDisplay = (data ?? [])
    .sort((a, b) => a.callSign.localeCompare(b.callSign))
    .slice(page * PageSize, (page + 1) * PageSize);
  const showPagination = data.length > PageSize;

  return (
    <>
      <h1 className="title">Vehicle Status</h1>
      <PlacePicker />
      <div className="table-container">
        <table className="table is-striped is-fullwidth">
          <thead>
            <tr>
              <th className="call-sign-col">Call Sign</th>
              <th className="reg-col">Registration</th>
              <th className="vor-col">Is VOR?</th>
              <th className="due-back-col">Expected Back</th>
              <th>Summary</th>
            </tr>
          </thead>
          <tbody>
            {itemsToDisplay.length > 0 &&
              itemsToDisplay.map((i) => (
                <tr key={i.id}>
                  <td className="call-sign-col" title={i.callSign}>
                    {i.callSign}
                  </td>
                  <td className="reg-col">{i.registration}</td>
                  <td className="vor-col">
                    <span
                      className={i.isVor ? "tag is-danger" : "tag is-success"}
                    >
                      {i.isVor ? "Yes" : "No"}
                    </span>
                  </td>
                  <td className="due-back-col">{i.dueBack}</td>
                  <td className="summary" title={i.summary}>
                    {i.summary}
                  </td>
                </tr>
              ))}

            {itemsToDisplay.length === 0 && (
              <tr>
                <td colSpan={5} className="no-vehicles subtitle is-6">
                  No Vehicles
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
      {showPagination && (
        <PagePicker
          page={page}
          pages={Math.ceil(data.length / PageSize)}
          setPage={setPage}
        />
      )}
    </>
  );
};

export const Route = createFileRoute("/vehicles/status")({
  beforeLoad: ({ context, search }) => {
    if (!context.loggedIn) {
      throw redirect({
        to: "/",
      });
    }
    if (!context.canViewVor) {
      throw redirect({
        to: "/home",
        search: search,
      });
    }
  },
  validateSearch: (
    search: Record<string, unknown>,
  ): { region: string; district: string; hub: string } => {
    return {
      region: search.region as string,
      district: search.district as string,
      hub: search.hub as string,
    };
  },
  loaderDeps: ({ search: { region, district, hub } }) => ({
    region,
    district,
    hub,
  }),
  loader: ({ deps, context }) => {
    return context.queryClient.ensureQueryData(
      statusOptions(deps.region, deps.district, deps.hub),
    );
  },
  component: VehicleStatus,
});
