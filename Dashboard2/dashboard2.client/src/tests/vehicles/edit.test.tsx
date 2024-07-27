import { useNavigate } from "@tanstack/react-router";
import { act, render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import ReactDOM from "react-dom/client";
import { Mock, beforeEach, describe, expect, it, vi } from "vitest";

import {
  UpdateVehicleSettings,
  VehicleSettings,
  useUpdateVehicle,
  useVehicleSettings,
} from "../../queries/vehicle-queries";
import { EditVehicle } from "../../routes/vehicles/edit.$id";

vi.mock("../../queries/vehicle-queries");
vi.mock("../../queries/user-queries");

const testItem: VehicleSettings = {
  id: "d",
  callSign: "WR123",
  district: "Avon",
  forDisposal: false,
  hub: "Bristol",
  region: "SouthWest",
  registration: "AB12 HTE",
  vehicleType: "AllWheelDrive",
};

const testUpdate: UpdateVehicleSettings = {
  callSign: "WR124",
  district: "Wiltshire",
  forDisposal: true,
  hub: "Bath",
  region: "SouthEast",
  registration: "AB12 HTE",
  vehicleType: "AllWheelDrive",
};

describe("Edit Vehicle Page", () => {
  beforeEach(() => {
    (useVehicleSettings as Mock).mockReturnValue({
      isLoading: false,
      data: testItem,
    });
    (useUpdateVehicle as Mock).mockReturnValue({
      mutateAsync: vi.fn(),
    });
  });

  it("renders without error", () => {
    const root = ReactDOM.createRoot(document.createElement("div"));
    expect(() => {
      act(() => {
        root.render(<EditVehicle id={testItem.id} />);
        root.unmount();
      });
    }).not.toThrow();
  });

  it("renders correctly", () => {
    const { asFragment } = render(<EditVehicle id={testItem.id} />);
    expect(asFragment()).toMatchSnapshot();

    expect(screen.getByLabelText(/call sign/i)).toHaveValue(testItem.callSign);
    expect(screen.getByLabelText(/registration/i)).toHaveValue(
      testItem.registration,
    );
    expect(screen.getByLabelText(/registration/i)).toBeDisabled();
    expect(screen.getByLabelText(/vehicle type/i)).toHaveValue(
      testItem.vehicleType,
    );
    expect(screen.getByLabelText(/region/i)).toHaveValue(testItem.region);
    expect(screen.getByLabelText(/district/i)).toHaveValue(testItem.district);
    expect(screen.getByLabelText(/hub/i)).toHaveValue(testItem.hub);
    expect(screen.getByLabelText(/for disposal/i)).not.toBeChecked();

    expect(useVehicleSettings).toBeCalledWith(testItem.id);
  });

  it("submits details to update api", async () => {
    const user = userEvent.setup();
    const mutateFn = vi.fn();
    const navigateFn = vi.fn();

    (useNavigate as Mock).mockReturnValue(navigateFn);
    (useUpdateVehicle as Mock).mockReturnValue({
      mutateAsync: mutateFn,
    });

    render(<EditVehicle id={testItem.id} />);

    await user.clear(screen.getByLabelText(/call sign/i));
    await user.type(screen.getByLabelText(/call sign/i), testUpdate.callSign);
    await user.selectOptions(
      screen.getByLabelText(/vehicle type/i),
      testUpdate.vehicleType,
    );
    await user.selectOptions(
      screen.getByLabelText(/region/i),
      testUpdate.region,
    );
    await user.clear(screen.getByLabelText(/district/i));
    await user.type(screen.getByLabelText(/district/i), testUpdate.district);
    await user.clear(screen.getByLabelText(/hub/i));
    await user.type(screen.getByLabelText(/hub/i), testUpdate.hub);
    await user.click(screen.getByLabelText(/for disposal/i));

    await user.click(screen.getByRole("button", { name: /save/i }));

    expect(screen.getByLabelText(/call sign/i)).toBeValid();
    expect(screen.getByLabelText(/vehicle type/i)).toBeValid();
    expect(screen.getByLabelText(/region/i)).toBeValid();
    expect(screen.getByLabelText(/district/i)).toBeValid();
    expect(screen.getByLabelText(/hub/i)).toBeValid();
    expect(mutateFn).toBeCalledWith(testUpdate);
    expect(navigateFn).toBeCalledWith({
      to: "/vehicles/config",
      search: {
        district: "All",
        hub: "All",
        region: "All",
      },
    });
  });

  it("disabled items when running", async () => {
    const user = userEvent.setup();
    const mutateFn = vi.fn().mockReturnValue(new Promise(() => {}));
    const navigateFn = vi.fn();

    (useNavigate as Mock).mockReturnValue(navigateFn);
    (useUpdateVehicle as Mock).mockReturnValue({
      mutateAsync: mutateFn,
    });

    render(<EditVehicle id={testItem.id} />);

    await user.clear(screen.getByLabelText(/call sign/i));
    await user.type(screen.getByLabelText(/call sign/i), testUpdate.callSign);
    await user.selectOptions(
      screen.getByLabelText(/vehicle type/i),
      testUpdate.vehicleType,
    );
    await user.selectOptions(
      screen.getByLabelText(/region/i),
      testUpdate.region,
    );
    await user.clear(screen.getByLabelText(/district/i));
    await user.type(screen.getByLabelText(/district/i), testUpdate.district);
    await user.clear(screen.getByLabelText(/hub/i));
    await user.type(screen.getByLabelText(/hub/i), testUpdate.hub);
    await user.click(screen.getByLabelText(/for disposal/i));

    await user.click(screen.getByRole("button", { name: /save/i }));

    expect(screen.getByLabelText(/call sign/i)).toBeDisabled();
    expect(screen.getByLabelText(/vehicle type/i)).toBeDisabled();
    expect(screen.getByLabelText(/region/i)).toBeDisabled();
    expect(screen.getByLabelText(/district/i)).toBeDisabled();
    expect(screen.getByLabelText(/hub/i)).toBeDisabled();
    expect(screen.getByLabelText(/for disposal/i)).toBeDisabled();
    expect(screen.getByRole("button", { name: /save/i })).toBeDisabled();
  });
});
