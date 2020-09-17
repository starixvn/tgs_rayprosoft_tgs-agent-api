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
  FieldStack,
  Dialog,
  Modal,
  Overlay,
  Portal
} from "bumbag"

const BumbagZIndex = 19900410

export default ({
  title,
  errors = [],
  visible = true,
  zIndex = BumbagZIndex,
  finalFocusRef,
  ...props
}) => {
  let zIndexString = `${zIndex + 100}`
  return (
    <Modal.State visible={visible} {...props}>
      <Modal.Backdrop zIndex={zIndexString}>
        <Dialog.Modal
          type="danger"
          variant="alert"
          title={title}
          placement="top"
          footer={
            <Flex
              alignX="right"
              css={css`
                width: 100%;
              `}
            >
              <Modal.Disclosure use={Button}>{close()}</Modal.Disclosure>
            </Flex>
          }
          hideOnClickOutside={false}
          unstable_finalFocusRef={finalFocusRef}
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
