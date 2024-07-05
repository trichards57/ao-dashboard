import { createFileRoute } from '@tanstack/react-router'
import validatePlace from '../../support/validate-place'

export const Route = createFileRoute('/vehicles/config')({
  validateSearch: validatePlace,
  component: () => <div>Hello /vehicles/config!</div>
})