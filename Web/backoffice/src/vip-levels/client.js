import useSWR from "swr"
import { useGet } from "../client"

const API = "/api/vipLevel"

export const useVipLevelList = () => {
  let { data, ...rest } = useGet(API)
  return {
    items: data?.vipLevels,
    ...rest
  }
}
