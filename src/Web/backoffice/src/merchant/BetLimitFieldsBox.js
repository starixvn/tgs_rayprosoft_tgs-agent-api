import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Box, Set, FieldStack } from "bumbag"
import { Trans, useTrans } from "../i18n"
import { NormalLabel } from "../form/Label"
import { useFormContext, useFieldArray } from "react-hook-form"
import { BetLimitCheckboxGroupTable } from "../betlimit/BetLimitCheckboxGroup"

export const BetLimitFieldsBox = ({ readonly }) => {
  let { watch, control } = useFormContext()

  let fieldsName = "betLimits"
  let fieldArrayContext = useFieldArray({
    control,
    name: fieldsName,
    keyName: "_id"
  })
  let betLimits = watch(fieldsName)

  if (betLimits.length == 0) {
    return null
  }

  return (
    betLimits && (
      <>
        <NormalLabel>
          <Trans zh="限红" en="Bet Limit" />
        </NormalLabel>
        <BetLimitCheckboxGroupTable
          fieldArrayContext={fieldArrayContext}
          disabled={readonly}
        />
      </>
    )
  )
}

export const toCheckedBetLimitIds = (languages = []) => {
  return languages
    .filter(({ checked }) => checked)
    .map(({ id }) => parseInt(id))
}

export default BetLimitFieldsBox
