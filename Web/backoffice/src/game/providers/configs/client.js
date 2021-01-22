import useSWR, { cache } from "swr"
import {
  urlWith,
  useGet,
  usePostData,
  usePut,
  usePutData,
  usePostFormData
} from "../../../client"

const API = "/api/GameProviderConfiguration"

export const useGameProviderConfigurationList = (searchParams) => {
  return {
    items: [
      {
        id: "CC8AA6E0-5A7F-44BC-B294-5097C3518C37",
        name: "Configuration SBO",
        description: "SBOBET",
        clientId: "SBO-WG",
        secretKey: "JFYif2fxc6jtOo3fA3JuaItNQ6SAlZAtNYGhgVmZGzm",
        isDefault: true
      }
    ],
    isLoading: false,
    error: undefined
  }

  let url = searchParams ? urlWith(searchParams, API) : ""
  let { data, ...rest } = useGet(url)
  return {
    items: data?.configurations ?? [],
    ...rest
  }
}
