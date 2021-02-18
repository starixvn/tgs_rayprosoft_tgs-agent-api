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
  let url = searchParams ? urlWith(searchParams, API) : ""
  let { data, ...rest } = useGet(url)
  return {
    items: data?.items ?? [],
    ...rest
  }
}
