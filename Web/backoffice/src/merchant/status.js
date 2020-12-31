import React, { useState, useEffect, useMemo } from "react"
import {
  css,
  Stack,
  Heading,
  Box,
  Button,
  Select as _Select,
  SelectMenu,
  DropdownMenu
} from "bumbag"
import { useForm, useFormContext, Controller } from "react-hook-form"
import Alert from "../dialog/Alert"
import { useTrans, Trans } from "../i18n"

const Active = "Active"
const ActiveTrans = {
  zh: "开启",
  en: "Enabled"
}

const Inactive = "Inactive"
const InactiveTrans = {
  zh: "关闭",
  en: Inactive
}

const Maintenance = "Maintenance"
const MaintenanceTrans = {
  zh: "维护",
  en: Maintenance
}

export const MerchantStatusLabel = ({ value = "" }) => {
  switch (value) {
    case Active:
      return <Trans {...ActiveTrans} />

    case Inactive:
      return <Trans {...InactiveTrans} />

    case Maintenance:
      return <Trans {...MaintenanceTrans} />

    default:
      return value
  }
}

const toPalette = (value) => {
  switch (value) {
    case Active:
      return "success"
    case Inactive:
      return "danger"
    case Maintenance:
      return "warning"
    default:
      return "default"
  }
}

export const MerchantStatusMenu = ({
  record = {},
  useChangeStatus,
  onSubmitted,
  ...props
}) => {
  let { id, status } = record
  let [newState, setSubmit] = useState()
  let { result, isLoading, error, deleteCache, mutate } = useChangeStatus?.(
    id,
    newState
  )

  const reset = () => {
    deleteCache()
    mutate(undefined, false)
    setSubmit(undefined)
  }

  useEffect(() => {
    if (result && !error) {
      reset()
      onSubmitted?.()
    }
  }, [result, error])

  return (
    <>
      <DropdownMenu
        menu={
          <>
            {status != Active && (
              <DropdownMenu.Item onClick={() => setSubmit(Active)}>
                <MerchantStatusLabel value={Active} />
              </DropdownMenu.Item>
            )}
            {status != Inactive && (
              <DropdownMenu.Item onClick={() => setSubmit(Inactive)}>
                <MerchantStatusLabel value={Inactive} />
              </DropdownMenu.Item>
            )}
            {status != Maintenance && (
              <DropdownMenu.Item onClick={() => setSubmit(Maintenance)}>
                <MerchantStatusLabel value={Maintenance} />
              </DropdownMenu.Item>
            )}
          </>
        }
      >
        <Button
          palette={toPalette(status)}
          isLoading={isLoading}
          disabled={isLoading}
          whiteSpace="nowrap"
        >
          <MerchantStatusLabel value={status} />
        </Button>
      </DropdownMenu>
    </>
  )
}
