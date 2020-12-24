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
import { useChangeGameProviderScheduleStatus } from "./client"
import Alert from "../../dialog/Alert"
import { useTrans, Trans } from "../../i18n"

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

export const MaintenanceStatusLabel = ({ value = "" }) => {
  switch (value) {
    case Active:
      return <Trans {...ActiveTrans} />

    case Inactive:
      return <Trans {...InactiveTrans} />

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
    default:
      return "default"
  }
}

export const MaintenanceStatusToggleButton = ({
  name = "",
  value,
  ...props
}) => {
  let { control, setValue } = useFormContext()
  return (
    <Controller
      control={control}
      name={name}
      render={(
        { onChange, onBlur, value, name, ref },
        { invalid, isTouched, isDirty }
      ) => {
        /*
        if (!value) {
          //default
          setValue(name, Active)
        }
        */
        return (
          <Button
            palette={toPalette(value)}
            onClick={(event) => {
              let status = value == Active ? Inactive : Active
              setValue(name, status)
            }}
          >
            <MaintenanceStatusLabel value={value} />
          </Button>
        )
      }}
    />
  )
}
