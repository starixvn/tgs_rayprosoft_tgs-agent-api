import useSWR from "swr"
import { useGet } from "../client"

export const useVipLevels = () => {
  let url = "/api/vipLevel"
  let { data, ...rest } = useGet(url)
  return {
    items: data?.vipLevels,
    ...rest
  }
}
