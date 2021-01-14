import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Table, Set, Heading, Text, Box } from "bumbag"
import { useFormContext, useFieldArray } from "react-hook-form"
import { isAllChecked } from "../../checkbox/isAllChecked"
import { Trans } from "../../i18n"
import { FieldArrayCheckboxGroup } from "../../checkbox/FieldArrayCheckboxGroup"

export const GameProviderCheckboxGroup = ({
  name = "gameProviderIds",
  fieldArrayName = "gameProviders",
  ...props
}) => (
  <FieldArrayCheckboxGroup
    name={name}
    fieldArrayName={fieldArrayName}
    {...props}
  />
)
