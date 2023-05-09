const cartRow = [...document.getElementsByClassName("cart-row")];

const setQuantity = async (id, quantity) => {
  const data = {
    id,
    quantity,
  };

  try {
    let result = await fetch("/cart/setquantity", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });
    if (!result.ok) {
      throw result;
    }
  } catch (err) {
    alert("FAILED");
  }
};

const isValid = (val) => {
  return !!val && val >= 1;
};

cartRow.forEach((cart) => {
  const cartQty = cart.querySelector("#cart-quantity");
  cartQty.addEventListener(
    "input",
    $.debounce(500, function () {
      const quantity = parseInt($(this).val());

      if (!isValid(quantity)) {
        return;
      }

      document.getElementById("total-price").innerText = cartRow.reduce(
        (acc, row) => {
          return (
            acc +
            +row.querySelector("#cart-quantity").value *
              +row.getAttribute("data-price")
          );
        },
        0
      );

      setQuantity(cart.getAttribute("data-id"), quantity);
    })
  );

  cartQty.addEventListener("change", function () {
    if (!isValid(cartQty.value)) {
      cartQty.value = 1;
      cartQty.dispatchEvent(new Event("input"));
    }
  });

  cart.querySelector("#increment").addEventListener("click", () => {
    cartQty.value = parseInt(cartQty.value, 10) + 1;
    cartQty.dispatchEvent(new Event("input"));
  });

  cart.querySelector("#decrement").addEventListener("click", () => {
    const val = parseInt(cartQty.value, 10);

    if (val > 1) {
      cartQty.value = parseInt(cartQty.value, 10) - 1;
      cartQty.dispatchEvent(new Event("input"));
    }
  });
});
