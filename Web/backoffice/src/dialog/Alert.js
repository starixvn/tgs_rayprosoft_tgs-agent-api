import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import {
  Spinner,
  Stack,
  Flex,
  Box,
  Card,
  Set,
  Button,
  Text,
  Label,
  Input,
  InputField,
  Checkbox,
  FieldStack,
  SelectField,
  Dialog,
  Modal,
  Overlay,
  Portal
} from "bumbag"

export default ({ title, errors = [], zIndex, disclosureRef, ...props }) => {
  return (
    <Modal.State {...props}>
      <Modal.Backdrop zIndex={zIndex}>
        <Dialog.Modal
          type="danger"
          variant="alert"
          title={title}
          placement="top"
          footer={() => (
            <Flex
              alignX="right"
              css={css`
                width: 100%;
              `}
            >
              <Modal.Disclosure use={Button}>{close()}</Modal.Disclosure>
            </Flex>
          )}
          hideOnClickOutside={false}
          unstable_finalFocusRef={disclosureRef}
        >
          <Box>
            {errors &&
              errors.map((error, index) => (
                <Text.Block key={index}>{error}</Text.Block>
              ))}
          </Box>
        </Dialog.Modal>
      </Modal.Backdrop>
    </Modal.State>
  )
}

//TODO: i18n
const close = () => "关闭"
