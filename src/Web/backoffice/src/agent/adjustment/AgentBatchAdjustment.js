import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/react"
import {
    Spinner,
    Stack,
    Flex,
    Box,
    Card,
    Set,
    Button,
    Text,
    Table,
    Icon
} from "bumbag"
import { faMinus, faCog, faPlus } from "@fortawesome/free-solid-svg-icons"
import { FieldValues, TwoColumns } from "../../layout/Grid"
import { Trans, FormattedDateTime, FormattedNumber, useTrans } from "../../i18n"
import AlertDialog from "../../dialog/Alert"
import { SubmitCancel } from "../../button/ActionButtons"
import { Modal, useModalState } from "../../dialog/Modal"
import { FormProvider, useForm, useFormContext } from "react-hook-form"
import Input from "../../input/Input"
import { CancelText, SubmitText } from "../../button/ActionButtons"
import Backdrop from "../../dialog/Backdrop"
import FormDialog from "../../form/FormDialog"
import Disclosure from "../../dialog/Disclosure"

//TODO
export const BatchAdjustmentDisclosure = ({}) => {}

const BatchAdjustmentDialog = ({}) => {}
