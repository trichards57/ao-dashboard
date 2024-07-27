import { render } from "@testing-library/react";
import { act } from "react";
import ReactDOM from "react-dom/client";
import { Mock, beforeEach, describe, expect, it, vi } from "vitest";

import { useDistricts, useHubs } from "../queries/place-queries";
import { VorStatistics, useStatistics } from "../queries/vor-queries";
import { Home } from "../routes/home";

vi.mock("../queries/vor-queries");
vi.mock("../queries/place-queries");

const testItem: VorStatistics = {
  availableVehicles: 10,
  vorVehicles: 5,
  totalVehicles: 15,
  pastAvailability: {
    "2020-01-01": 10,
    "2020-02-01": 11,
    "2020-03-01": 12,
    "2020-04-01": 13,
    "2020-05-01": 14,
    "2020-06-01": 15,
    "2020-07-01": 14,
    "2020-08-01": 13,
    "2020-09-01": 12,
    "2020-10-01": 11,
    "2020-11-01": 10,
    "2020-12-01": 9,
  },
};

const testDistricts = ["Avon", "Bedfordshire", "Berkshire", "Buckinghamshire"];
const testHubs = ["Bristol", "Bath", "Weston-super-Mare", "Yeovil"];

describe("Home Component", () => {
  beforeEach(() => {
    (useStatistics as Mock).mockReturnValue({
      data: testItem,
    });
    (useDistricts as Mock).mockReturnValue({
      data: testDistricts,
    });
    (useHubs as Mock).mockReturnValue({
      data: testHubs,
    });
  });

  it("renders without error", () => {
    const root = ReactDOM.createRoot(document.createElement("div"));
    expect(() => {
      act(() => {
        root.render(<Home region="All" district="All" hub="All" />);
        root.unmount();
      });
    }).not.toThrow();
  });

  it("renders correctly", () => {
    const { asFragment } = render(
      <Home region="SouthWest" district="Avon" hub="Bristol" />,
    );
    expect(asFragment()).toMatchSnapshot();
    expect(useStatistics).toHaveBeenCalledWith("SouthWest", "Avon", "Bristol");
    expect(useDistricts).toHaveBeenCalledWith("SouthWest");
    expect(useHubs).toHaveBeenCalledWith("SouthWest", "Avon");
  });
});
