import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
  useModalState,
  Stack,
  Heading,
  Box,
  Text,
  FieldStack,
  Card,
  Set,
  Table,
  Button
} from "bumbag"
import { getTrans, Trans, useLocale, useTrans } from "../../i18n"
import {
  useFormContext,
  useFieldArray,
  Controller,
  useForm
} from "react-hook-form"
import { Switch, SwitchField } from "../../switch/Switch"

export const SportsSwitch = ({ name = "", ...props }) => {
  let { watch } = useFormContext()

  let isSports = watch(name)
  if (!name && typeof props?.checked == "boolean") {
    isSports = props?.checked
  }

  let sportLabel = useTrans({
    zh: "体育",
    en: "Sports"
  })

  let notSportLabel = useTrans({
    zh: "非体育",
    en: "Not Sports"
  })

  return useMemo(
    () => (
      <Switch
        name={name}
        label={isSports ? sportLabel : notSportLabel}
        {...props}
      />
    ),
    [isSports]
  )
}
