/* prettier-ignore-start */

/* eslint-disable */

// @ts-nocheck

// noinspection JSUnusedGlobalSymbols

// This file is auto-generated by TanStack Router

import { createFileRoute } from '@tanstack/react-router'

// Import Routes

import { Route as rootRoute } from './routes/__root'

// Create Virtual Routes

const HomeLazyImport = createFileRoute('/home')()
const IndexLazyImport = createFileRoute('/')()
const VehiclesStatusLazyImport = createFileRoute('/vehicles/status')()
const VehiclesConfigLazyImport = createFileRoute('/vehicles/config')()

// Create/Update Routes

const HomeLazyRoute = HomeLazyImport.update({
  path: '/home',
  getParentRoute: () => rootRoute,
} as any).lazy(() => import('./routes/home.lazy').then((d) => d.Route))

const IndexLazyRoute = IndexLazyImport.update({
  path: '/',
  getParentRoute: () => rootRoute,
} as any).lazy(() => import('./routes/index.lazy').then((d) => d.Route))

const VehiclesStatusLazyRoute = VehiclesStatusLazyImport.update({
  path: '/vehicles/status',
  getParentRoute: () => rootRoute,
} as any).lazy(() =>
  import('./routes/vehicles/status.lazy').then((d) => d.Route),
)

const VehiclesConfigLazyRoute = VehiclesConfigLazyImport.update({
  path: '/vehicles/config',
  getParentRoute: () => rootRoute,
} as any).lazy(() =>
  import('./routes/vehicles/config.lazy').then((d) => d.Route),
)

// Populate the FileRoutesByPath interface

declare module '@tanstack/react-router' {
  interface FileRoutesByPath {
    '/': {
      preLoaderRoute: typeof IndexLazyImport
      parentRoute: typeof rootRoute
    }
    '/home': {
      preLoaderRoute: typeof HomeLazyImport
      parentRoute: typeof rootRoute
    }
    '/vehicles/config': {
      preLoaderRoute: typeof VehiclesConfigLazyImport
      parentRoute: typeof rootRoute
    }
    '/vehicles/status': {
      preLoaderRoute: typeof VehiclesStatusLazyImport
      parentRoute: typeof rootRoute
    }
  }
}

// Create and export the route tree

export const routeTree = rootRoute.addChildren([
  IndexLazyRoute,
  HomeLazyRoute,
  VehiclesConfigLazyRoute,
  VehiclesStatusLazyRoute,
])

/* prettier-ignore-end */