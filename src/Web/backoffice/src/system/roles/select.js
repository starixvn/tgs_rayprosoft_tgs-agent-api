import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { SelectField, Dialog, Modal, Table, useClassName } from "bumbag"
import AlertDialog from "../../dialog/Alert"
import { useRoleList } from "./client"
import {
  useUserBrandId,
  useUserLicenseeId,
  useUserResellerId,
  useUserRoleType
} from "../../user/context"
import Select from "../../select/Select"

export const RoleSelect = ({ ...props }) => {
  let userResellerId = useUserResellerId()
  let userLicenseeId = useUserLicenseeId()
  let userBrandId = useUserBrandId()

  let { error, data = {}, isLoading } = useRoleList({
    showAll: true,
    resellerId: userResellerId,
    licenseeId: userLicenseeId,
    brandId: userBrandId
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
