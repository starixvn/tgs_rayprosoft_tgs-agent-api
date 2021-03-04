import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import AlertDialog from "../../dialog/Alert"
import { useDepartmentList } from "./client"
import { Select } from "../../select/Select"
import {
  useUserBrandId,
  useUserLicenseeId,
  useUserResellerId
} from "../../user/context"

export const DepartmentSelect = ({ ...props }) => {
  let resellerId = useUserResellerId()
  let licenseeId = useUserLicenseeId()
  let brandId = useUserBrandId()

  let { error, data = {}, isLoading } = useDepartmentList({
    showAll: true,
    resellerId,
    licenseeId,
    brandId
  })
  let { items = [] } = data
  let options = items.map(({ id, name }) => ({
    label: name,
    value: id
  }))

  return (
    <>
      <Select isLoading={isLoading} options={options} {...props} />
      {error && <AlertDialog {...error} />}
    </>
  )
}
