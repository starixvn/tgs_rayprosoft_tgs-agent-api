import React, { useState } from "react"
import { PageContent, Alert } from "bumbag"

const DefaultTitle = "Page Not Found"

export default ({ title = DefaultTitle }) => (
  <PageContent>
    <Alert variant="bordered" title={title} type="warning"></Alert>
  </PageContent>
)
