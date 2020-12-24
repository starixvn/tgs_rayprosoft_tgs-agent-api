import useSWR, { cache } from "swr"
import {
  urlWith,
  useGet,
  usePostData,
  usePut,
  usePutData,
  usePostFormData
} from "../../client"

const API = "/api/brandgameprovider"

export const useBrandGameProviderList = (searchParams) => {
  let url = searchParams ? urlWith(searchParams, API) : ""
  return useGet(url)
}

export const useUpdateBrandGameProvider = (id, data) => {
  let url = id ? `${API}/${id}` : ""
  return usePutData(url, data)
}

export const useBrandGameProviderPresetMaintenanceList = (id) => {
  let url = id ? `${API}/${id}/defaultTimes` : ""
  let { data, ...result } = useGet(url)

  return {
    items: data?.defaultTimes,
    ...result
  }
}

export const useUpdateBrandGameProviderPresetMaintenance = (id, data) => {
  let url = id ? `${API}/${id}/defaultTimes` : ""
  return usePutData(url, data)
}

export const useChangeBrandGameProviderScheduleStatus = (id, status) => {
  let url = id && status ? `${API}/${id}/${status}` : ""
  return usePut(url)
}
