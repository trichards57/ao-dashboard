import { afterEach, vi } from "vitest";
import { cleanup } from "@testing-library/react";
import "vitest-canvas-mock";

vi.mock("@tanstack/react-router", () => ({
  Link: "a",
  createFileRoute: vi.fn().mockReturnValue(vi.fn()),
  useNavigate: vi.fn().mockReturnValue(vi.fn()),
}));

// Mock the ResizeObserver
const ResizeObserverMock = vi.fn(() => ({
  observe: vi.fn(),
  unobserve: vi.fn(),
  disconnect: vi.fn(),
}));

// Stub the global ResizeObserver
vi.stubGlobal("ResizeObserver", ResizeObserverMock);

afterEach(() => {
  cleanup();
});
