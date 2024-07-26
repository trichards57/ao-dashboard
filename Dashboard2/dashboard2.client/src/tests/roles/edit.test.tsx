import { describe, expect, it, Mock, vi, beforeEach } from "vitest";
import {
  RolePermissions,
  RolePermissionsUpdate,
  useRole,
  useUpdateRole,
} from "../../queries/role-queries";
import { act, render, screen } from "@testing-library/react";
import ReactDOM from "react-dom/client";
import { EditRole } from "../../routes/roles/edit.$id";
import userEvent from "@testing-library/user-event";
import { useNavigate } from "@tanstack/react-router";

vi.mock("../../queries/role-queries");

const testItem: RolePermissions = {
  id: "b",
  name: "Role 1",
  permissions: "Deny",
  vehicleConfiguration: "Read",
  vorData: "Read",
};

const testUpdate: RolePermissionsUpdate = {
  permissions: "Write",
  vehicleConfiguration: "Deny",
  vorData: "Deny",
};

describe("Edit Role Page", () => {
  beforeEach(() => {
    (useRole as Mock).mockReturnValue({
      isLoading: false,
      data: testItem,
    });
    (useUpdateRole as Mock).mockReturnValue({
      mutateAsync: vi.fn(),
    });
  });

  it("renders without error", () => {
    const root = ReactDOM.createRoot(document.createElement("div"));
    expect(() => {
      act(() => {
        root.render(<EditRole id={testItem.id} />);
        root.unmount();
      });
    }).not.toThrow();
  });

  it("renders correctly", () => {
    const { asFragment } = render(<EditRole id={testItem.id} />);
    expect(asFragment()).toMatchSnapshot();

    expect(screen.getByLabelText(/vehicle configuration/i)).toHaveValue(
      testItem.vehicleConfiguration,
    );
    expect(screen.getByLabelText(/vor data/i)).toHaveValue(testItem.vorData);
    expect(screen.getByLabelText(/permissions/i)).toHaveValue(
      testItem.permissions,
    );
    expect(useRole).toBeCalledWith(testItem.id);
  });

  it("submits details to update api", async () => {
    const user = userEvent.setup();
    const mutateFn = vi.fn();
    const navigateFn = vi.fn();

    (useNavigate as Mock).mockReturnValue(navigateFn);
    (useUpdateRole as Mock).mockReturnValue({
      mutateAsync: mutateFn,
    });

    render(<EditRole id={testItem.id} />);

    await user.selectOptions(
      screen.getByLabelText(/vehicle configuration/i),
      testUpdate.vehicleConfiguration,
    );
    await user.selectOptions(
      screen.getByLabelText(/vor data/i),
      testUpdate.vorData,
    );
    await user.selectOptions(
      screen.getByLabelText(/permissions/i),
      testUpdate.permissions,
    );

    await user.click(screen.getByRole("button", { name: /save/i }));

    expect(screen.getByLabelText(/vehicle configuration/i)).toBeValid();
    expect(screen.getByLabelText(/vor data/i)).toBeValid();
    expect(screen.getByLabelText(/permissions/i)).toBeValid();
    expect(mutateFn).toBeCalledWith(testUpdate);
    expect(navigateFn).toBeCalledWith({
      to: "/roles",
    });
  });

  it("disabled items when running", async () => {
    const user = userEvent.setup();
    const mutateFn = vi.fn().mockReturnValue(new Promise(() => {}));
    const navigateFn = vi.fn();

    (useNavigate as Mock).mockReturnValue(navigateFn);
    (useUpdateRole as Mock).mockReturnValue({
      mutateAsync: mutateFn,
    });

    render(<EditRole id={testItem.id} />);

    await user.selectOptions(
      screen.getByLabelText(/vehicle configuration/i),
      testUpdate.vehicleConfiguration,
    );
    await user.selectOptions(
      screen.getByLabelText(/vor data/i),
      testUpdate.vorData,
    );
    await user.selectOptions(
      screen.getByLabelText(/permissions/i),
      testUpdate.permissions,
    );

    await user.click(screen.getByRole("button", { name: /save/i }));

    expect(screen.getByLabelText(/vehicle configuration/i)).toBeDisabled();
    expect(screen.getByLabelText(/vor data/i)).toBeDisabled();
    expect(screen.getByLabelText(/permissions/i)).toBeDisabled();
    expect(screen.getByRole("button", { name: /save/i })).toBeDisabled();
  });
});
