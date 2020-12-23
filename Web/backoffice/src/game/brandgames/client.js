import useSWR, { cache } from "swr"
import {
  urlWith,
  useGet,
  usePostData,
  usePut,
  usePutData,
  usePostFormData
} from "../../client"

const API = "/api/brandgame"

export const useBrandGameList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}

export const useBrandGame = (id) => {
  let url = id ? `${API}/${id}` : undefined
  return useGet(url)
}

export const useUpdateBrandGame = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return usePutData(url, data)
}

export const useUpdateBrandGameTag = (data) => {
  let url =
    data?.id && data?.tag
      ? `${API}/${id}/${data.tag}/${Boolean(data.enabled).toString()}`
      : undefined

  return usePut(url)
}

export const useUpdateBrandGameStatus = () => {
  console.warn("TODO: useUpdateBrandGameStatus")
}
