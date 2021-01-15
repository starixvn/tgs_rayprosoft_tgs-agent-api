import useSWR, { cache } from "swr"
import {
  urlWith,
  useGet,
  usePostData,
  usePut,
  usePutData,
  usePostFormData
} from "../../client"

const API = "/api/licensee"

export const useLicenseeList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}

export const useLicensee = (id) => {
  let url = id ? `${API}/${id}` : ""
  let { data, ...rest } = useGet(url)

  return {
    data: data?.licensee,
    ...rest
  }
}

export const useAddLicensee = (data) => {
  let url = data ? API : ""
  return usePostData(url, data)
}

export const useUpdateLicensee = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return usePutData(url, data)
}

export const useChangeLicenseeStatus = (id, status) => {
  let url = id && status ? `${API}/${id}/${status}` : ""
  return usePut(url)
}
