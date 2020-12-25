import React, { useState, useEffect, useMemo } from "react"
import {
  css,
  Stack,
  Heading,
  Box,
  Button,
  Select,
  SelectMenu,
  DropdownMenu,
  Switch
} from "bumbag"
import { useTrans, Trans } from "../../i18n"

const New = "New"
const Hot = "Hot"
const Recommend = "Recommend"

export const GameTag = {
  New,
  Hot,
  Recommend
}
