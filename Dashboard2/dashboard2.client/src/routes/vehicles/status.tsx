import { createFileRoute } from "@tanstack/react-router";
import PlacePicker from "../../components/place-picker";
import { preloadStatus, useStatus } from "../../queries/vor-queries";
import { useState } from "react";
import PagePicker from "../../components/page-picker";
import validatePlace, { getPlaceDeps } from "../../support/validate-place";
import { useTitle } from "../../components/useTitle";
import {
  preloadDistricts,
  preloadHubs,
  Region,
} from "../../queries/place-queries";
import {
  redirectIfLoggedOut,
  redirectIfNoPermission,
} from "../../support/check-logged-in";

const PageSize = 10;

function VehicleStatus({
  region,
  district,
  hub,
}: {
  region: Region;
  district: string;
  hub: string;
}) {
  const { data } = useStatus(region, district, hub);
  const [page, setPage] = useState(0);

  useTitle("Vehicle Status");

  const itemsToDisplay = (data ?? [])
    .toSorted((a, b) => a.callSign.localeCompare(b.callSign))
    .slice(page * PageSize, (page + 1) * PageSize);
  const showPagination = data.length > PageSize;

  return (
    <>
      <h1 className="title">Vehicle Status</h1>
      <PlacePicker region={region} district={district} hub={hub} />
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
}

export const Route = createFileRoute("/vehicles/status")({
  beforeLoad: ({ context, search }) => {
    redirectIfLoggedOut(context);
    redirectIfNoPermission(context.canViewVor, search);
  },
  validateSearch: validatePlace,
  loaderDeps: getPlaceDeps,
  loader: ({ deps, context }) =>
    Promise.all([
      preloadStatus(context.queryClient, deps.region, deps.district, deps.hub),
      preloadDistricts(context.queryClient, deps.region),
      preloadHubs(context.queryClient, deps.region, deps.district),
    ]),
  component: function Component() {
    return (
      <VehicleStatus
        region={Route.useSearch().region}
        district={Route.useSearch().district}
        hub={Route.useSearch().hub}
      />
    );
  },
});
